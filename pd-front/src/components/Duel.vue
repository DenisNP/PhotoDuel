<template>
    <div class="duel-container">
        <challenge-lite class="challenge" :challenge-id="duel.challengeId"/>
        <div class="draw-container" v-show="showDrawContainer">
            <challenge-lite
                class="draw-challenge"
                ref="challengeContainer"
                :challenge-id="duel.challengeId"
            />
        </div>
        <f7-button icon-material="report" color="gray" class="report-btn" @click="report"/>
        <div class="photo-block">
            <div class="duellist">
                <div class="photo">
                    <f7-icon material="check_circle" class="voted-icon" v-if="voteCreator"/>
                    <img :src="duel.creator.image"/>
                </div>
                <user class="user" :user="duel.creator.user"/>
            </div>
            <div class="middle">
                <div class="middle-top">
                    <div class="revolvers-icon"></div>
                    <div class="timer">{{timerText}}</div>
                </div>
                <div class="votes">
                    {{duel.creator.voters.length}}
                    :
                    {{duel.opponent ? duel.opponent.voters.length : 0}}
                </div>
            </div>
            <div class="duellist">
                <div class="photo">
                    <f7-icon material="check_circle" class="voted-icon" v-if="voteOpponent"/>
                    <img v-if="duel.opponent" :src="duel.opponent.image"/>
                </div>
                <user class="user" v-if="duel.opponent" :user="duel.opponent.user"/>
            </div>
        </div>
        <div class="buttons-block" v-if="isCreated || isCurrent">
            <f7-button
                v-if="isCreated && isMine && !duel.isPublic && !hasOtherCurrent"
                fill
                class="duel-btn"
                icon-material="public"
                icon-size="20"
                @click="publish"
            >
                Публичный вызов
            </f7-button>
            <f7-button
                v-if="isCreated && isMine && !duel.isPublic"
                fill
                class="duel-btn"
                icon-material="group"
                icon-size="20"
                @click="inviteFriends"
            >
                Вызов друзьям
            </f7-button>
            <f7-button
                v-if="isCreated && isMine && !duel.isPublic"
                fill
                class="duel-btn"
                icon-material="share"
                icon-size="20"
                @click="sendLink"
            >
                Персональный вызов
            </f7-button>
            <f7-button
                v-if="isCreated && !isMine && !hasOtherCurrent"
                fill
                class="duel-btn"
                icon-material="local_fire_department"
                icon-size="20"
                @click="join"
            >
                Принять вызов
            </f7-button>
            <f7-button
                v-if="isCurrent"
                class="duel-btn"
                icon-material="play_circle_filled"
                icon-size="20"
                fill
                @click="sendVoting"
            >
                Запустить голосование
            </f7-button>
            <f7-button
                v-if="isCreated && !isMine"
                class="duel-btn-empty"
                icon-material="clear"
                icon-size="20"
                @click="reject"
            >
                Отклонить вызов
            </f7-button>
            <f7-button
                v-if="isCreated && isMine"
                class="duel-btn-empty"
                icon-material="clear"
                icon-size="20"
                @click="deleteDuel"
            >
                Удалить дуэль
            </f7-button>
        </div>
    </div>
</template>

<script>
import VKC from '@denisnp/vkui-connect-helper';
import ChallengeLite from './ChallengeLite.vue';
import User from './User.vue';
import { getAppId } from '../common/utils';
import api from '../common/api';
import { createSoloStory, createVoteStory } from '../common/storyCreator';

export default {
    name: 'Duel',
    data() {
        return {
            timer: -1,
            timeLeft: 0,
            showDrawContainer: false,
        };
    },
    computed: {
        timerText() {
            if (this.duel.status === 'Finished') return 'Завершено';
            if (this.duel.status === 'Created') {
                if (this.duel.isPublic) return 'Публичная: ожидание';
                return 'Отправьте вызов';
            }

            const hours = Math.floor(this.timeLeft / 3600);
            const minutes = Math.floor((this.timeLeft - hours * 3600) / 60);
            const seconds = this.timeLeft - hours * 3600 - minutes * 60;

            return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        },
        isMine() {
            return this.duel.creator.user.id === this.$store.state.user.id;
        },
        participateIn() {
            return this.isMine
                || (
                    this.duel.opponent
                    && this.duel.opponent.user.id === this.$store.state.user.id
                );
        },
        isCurrent() {
            return this.duel.status === 'Started' && this.participateIn;
        },
        hasOtherCurrent() {
            return this.$store.getters.currentDuels.length > 1;
        },
        isCreated() {
            return this.duel.status === 'Created';
        },
        voteCreator() {
            return this.duel.creator.voters.some((v) => v.id === this.$store.state.user.id);
        },
        voteOpponent() {
            return this.duel.opponent !== null
                && this.duel.opponent.voters.some((v) => v.id === this.$store.state.user.id);
        },
    },
    methods: {
        showHelp(title, text, onYes, okText) {
            this.$f7.dialog.confirm(
                text,
                title,
                async () => {
                    const success = await onYes();
                    if (!success) return;
                    this.$f7.toast.create({
                        text: okText || 'Готово!',
                        position: 'center',
                        cssClass: 'my-text-center',
                        closeTimeout: 1500,
                    }).open();
                },
            );
        },
        calculateTime() {
            this.timeLeft = Math.floor((this.duel.timeFinish - (new Date()).getTime()) / 1000);
        },
        report() {
            this.$f7.dialog.confirm(
                'Отправить жалобу на неподобающий контент или нарушение правил в этой дуэли?',
                'Жалоба',
                () => {
                    api('report', { duelId: this.duel.id });

                    this.$f7.toast.create({
                        text: 'Спасибо! Модераторы рассмотрят вашу жалобу!',
                        position: 'center',
                        cssClass: 'my-text-center',
                        closeTimeout: 1500,
                    }).open();
                },
            );
        },
        publish() {
            this.showHelp(
                'Публичный вызов',
                'Ваш вызов будет предложен случайному пользователю приложения. Продолжаем?',
                async () => {
                    const success = await this.$store.dispatch('publish', this.duel.id);
                    if (success) this.$store.commit('setTab', 2);
                    return success && !this.requestNotify();
                },
                'Теперь дождитесь, когда ваш вызов увидят и примут.',
            );
        },
        inviteFriends() {
            this.$store.commit('setLoading', true);
            this.showDrawContainer = true;
            this.$nextTick(async () => {
                const storyData = await createSoloStory(
                    this.duel.creator.image,
                    this.$refs.challengeContainer.$el,
                    this.duel.id,
                );
                await VKC.send('VKWebAppShowStoryBox', storyData);
                this.showDrawContainer = false;
                this.$store.commit('setLoading', false);
            });
            return false;
        },
        sendLink() {
            this.showHelp(
                'Персональный вызов',
                'Выбранный вами друг получит прямую ссылку на дуэль и сможет принять вызов. Продолжить?',
                async () => {
                    const [result] = await VKC.send(
                        'VKWebAppShare',
                        { link: `https://vk.com/app${getAppId()}#${this.duel.id}` },
                    );
                    return result && !this.requestNotify();
                },
                'Теперь дождитесь, когда друг примет вызов',
            );
        },
        join() {
            this.$f7.views.main.router.navigate(`/duel/${this.duel.id}`);
        },
        sendVoting() {
            this.$store.commit('setLoading', true);
            this.showDrawContainer = true;
            this.$nextTick(async () => {
                const storyData = await createVoteStory(
                    this.duel.creator.image,
                    this.duel.opponent.image,
                    this.$refs.challengeContainer.$el,
                    this.duel.id,
                );
                await VKC.send('VKWebAppShowStoryBox', storyData);
                // TODO send story
                this.showDrawContainer = false;
                this.$store.commit('setLoading', false);
            });
            return false;
        },
        reject() {
            this.showHelp(
                'Отклонить вызов',
                'Вызов будет отклонён. Продолжаем?',
                () => this.$store.dispatch('init', true),
            );
        },
        deleteDuel() {
            this.showHelp(
                'Удалить дуэль',
                'Вызов будет необратимо удалён. Продолжаем?',
                () => this.$store.dispatch('deleteDuel', this.duel.id),
            );
        },
        requestNotify() {
            if (this.$store.notifications) return false;
            this.$store.commit('setNotifications', true);

            this.$f7.dialog.confirm(
                'Активируйте уведомления, чтобы узнать, когда ваш вызов примут.',
                'Отправка вызова',
                () => VKC.send('VKWebAppAllowNotifications', {}),
            );
            return true;
        },
    },
    components: {
        User,
        ChallengeLite,
    },
    mounted() {
        clearInterval(this.timer);
        if (this.duel.status === 'Finished') return;

        // create timer
        this.timer = setInterval(() => {
            this.calculateTime();
        }, 1000);
        this.calculateTime();
    },
    beforeDestroy() {
        clearInterval(this.timer);
    },
    props: {
        duel: {
            type: Object,
            required: true,
        },
    },
};
</script>

<style scoped>
    .duel-container {
        width: calc(100% - 20px);
        margin-left: auto;
        margin-right: auto;
        background: white;
        box-shadow: 0 1px 4px 0 rgba(0,0,0,0.35);
        position: relative;
    }

    .challenge {
        padding: 10px;
    }

    .photo-block {
        display: flex;
        justify-content: space-between;
        padding: 5px 15px 15px 15px;
    }

    .duellist {
        display: flex;
        flex-direction: column;
        /*align-items: center;*/
    }

    .photo {
        width: 32vw;
        height: 32vw;
        margin-bottom: 10px;
        position: relative;
        background: #ddd;
    }

    .photo > img {
        width: 100%;
        height: 100%;
    }

    .user {
        margin-left: 5px;
    }

    .middle-top {
        height: calc(32vw + 15px);
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }

    .votes {
        color: darkgreen;
        font-size: 18px;
        font-weight: bold;
        text-align: center;
    }

    .revolvers-icon {
        background-image: url("../assets/revolvers.svg");
        background-size: contain;
        width: 50px;
        height: 50px;
        opacity: 0.25;
    }

    .timer {
        margin-top: 10px;
        color: darkgreen;
        text-align: center;
        font-size: 12px;
    }

    .report-btn {
        position: absolute;
        top: 0;
        right: 0;
        opacity: 0.5;
        margin-right: -5px;
    }

    .buttons-block {
        padding: 0 15px 15px 15px;
    }

    .duel-btn {
        margin-bottom: 7px;
    }

    .voted-icon {
        position: absolute;
        bottom: -8px;
        right: -8px;
        color: darkgreen;
        background: white;
        border-radius: 50%;
    }

    .draw-container {
        width: 0;
        height: 0;
        overflow: hidden;
    }

    .draw-challenge {
        width: 335px;
        height: 57px;
        padding: 0!important;
        margin: 0!important;
    }
</style>

<style>
    .duel-btn .icon, .duel-btn-empty .icon {
        margin-top: -3px;
    }
</style>
