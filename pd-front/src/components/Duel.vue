<template>
    <div class="duel-container">
        <challenge-lite class="challenge" :challenge-id="duel.challengeId"/>
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
                class="play_circle_filled"
                icon-material="clear"
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
import ChallengeLite from './ChallengeLite.vue';
import User from './User.vue';

// публиный вызов - created, если ты создатель и нет текущей дуэли
// вызов друзьям - created, ты создатель, нет текущей дуэли
// отправить ссылку - created, ты создатель, нет текущей дуэли
// принять вызов - created, не ты создатель, нет текущей дуэли
// удалить - created, ты создатель
// отклонить - created, не ты создатель

export default {
    name: 'Duel',
    data() {
        return {
            timer: -1,
            timeLeft: 0,
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
        calculateTime() {
            this.timeLeft = Math.floor((this.duel.timeFinish - (new Date()).getTime()) / 1000);
        },
        report() {
            this.$f7.dialog.confirm(
                'Отправить жалобу на неподобающий контент или нарушение правил в этой дуэли?',
                'Жалоба',
                () => {
                    this.$f7.toast.create({
                        text: 'Спасибо! Модераторы рассмотрят вашу жалобу!',
                        position: 'center',
                        cssClass: 'my-text-center',
                        closeTimeout: 1500,
                    }).open();
                    // TODO call api
                },
            );
        },
        publish() {
            //
        },
        inviteFriends() {
            //
        },
        sendLink() {
            //
        },
        join() {
            //
        },
        sendVoting() {
            //
        },
        reject() {
            //
        },
        deleteDuel() {
            //
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
</style>

<style>
    .duel-btn .icon, .duel-btn-empty .icon {
        margin-top: -3px;
    }
</style>
