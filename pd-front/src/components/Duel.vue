<template>
    <div class="duel-container">
        <challenge-lite class="challenge" :challenge-id="duel.challengeId"/>
        <div class="photo-block">
            <div class="duellist">
                <div class="photo">
                    <img :src="duel.creator.image"/>
                </div>
                <user class="user" :user="duel.creator.user"/>
            </div>
            <div class="middle">
                <div class="middle-top">
                    <div class="icon"></div>
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
                    <img v-if="duel.opponent" :src="duel.opponent.image"/>
                </div>
                <user class="user" v-if="duel.opponent" :user="duel.opponent.user"/>
            </div>
        </div>
    </div>
</template>

<script>
import ChallengeLite from './ChallengeLite.vue';
import User from './User.vue';

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
            if (this.duel.status === 'Created') return 'Ожидание оппонента';

            const hours = Math.floor(this.timeLeft / 3600);
            const minutes = Math.floor((this.timeLeft - hours * 3600) / 60);
            const seconds = this.timeLeft - hours * 3600 - minutes * 60;

            return `${hours}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        },
    },
    methods: {
        calculateTime() {
            this.timeLeft = Math.floor((this.duel.timeFinish - (new Date()).getTime()) / 1000);
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

    .icon {
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
</style>
