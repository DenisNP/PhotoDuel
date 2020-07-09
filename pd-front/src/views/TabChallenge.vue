<template>
    <div class="full-height">
        <div v-if="!$store.getters.hasCreatedNonPublic" class="full-height">
            <div class="block-text">Выберите одно из заданий</div>
            <div class="challenge-lines">
                <challenge-line
                    :challenge-id="challengeIds[0]"
                    class="ch-line"
                    @go="createDuel(challengeIds[0])"
                />
                <challenge-line
                    :challenge-id="challengeIds[1]"
                    class="ch-line"
                    @go="createDuel(challengeIds[1])"
                />
                <challenge-line
                    :challenge-id="challengeIds[2]"
                    class="ch-line"
                    @go="createDuel(challengeIds[2])"
                />
            </div>
            <div v-if="publicDuel" class="block-text">Или примите вызов</div>
            <public-challenge
                class="public-challenge"
                v-if="publicDuel"
                :duel="publicDuel"
                @click.native="joinDuel(publicDuel.id)"
            />
            <f7-button outline color="white" class="shuffle-btn">
                Перемешать
            </f7-button>
            <div class="empty"/>
        </div>
        <div v-else>
            <duel
                v-for="d in $store.getters.currentDuels"
                :key="d.id"
                :duel="d"
                class="duel-block"
            />
            <div class="empty"/>
        </div>
    </div>
</template>

<script>
import ChallengeLine from '../components/ChallengeLine.vue';
import PublicChallenge from '../components/PublicChallenge.vue';
import Duel from '../components/Duel.vue';

export default {
    name: 'TabChallenge',
    computed: {
        challengeIds() {
            return this.$store.state.user.challengeIds;
        },
        publicDuel() {
            return this.$store.state.user.publicDuel;
        },
    },
    methods: {
        createDuel(challengeId) {
            this.$f7.views.main.router.navigate(`/duel/${challengeId}`);
        },
        joinDuel(duelId) {
            this.$f7.views.main.router.navigate(`/duel/${duelId}`);
        },
    },
    components: {
        Duel,
        ChallengeLine,
        PublicChallenge,
    },
};
</script>

<style scoped>
    .ch-line {
        margin-bottom: 15px;
    }

    .block-text {
        text-align: center;
        margin-bottom: 10px;
        color: white;
        font-size: 16px;
    }

    .public-challenge {
        width: calc(100vw - 60px);
        margin-left: auto;
        margin-right: auto;
    }

    .challenge-lines {
        width: 100%;
        overflow: hidden;
    }

    .shuffle-btn {
        margin: 30px auto 0 auto;
        width: 200px;
        height: 40px;
        padding-top: 5px;
        border-radius: 7px;
    }

    .md .shuffle-btn {
        padding-top: 2px;
    }

    .empty {
        height: 30px;
        width: 1px;
    }

    .duel-block {
        margin-bottom: 20px;
    }
</style>
