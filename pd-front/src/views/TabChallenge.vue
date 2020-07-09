<template>
    <div class="tab-challenge full-height">
        <div class="block-text">Выберите одно из заданий</div>
        <div class="challenge-lines">
            <challenge-line :challenge-id="$store.state.user.challengeIds[0]" class="ch-line"/>
            <challenge-line :challenge-id="$store.state.user.challengeIds[1]" class="ch-line"/>
            <challenge-line :challenge-id="$store.state.user.challengeIds[2]" class="ch-line"/>
        </div>
        <div v-if="$store.state.user.publicDuel" class="block-text">Или примите вызов</div>
        <public-challenge
            class="public-challenge"
            v-if="$store.state.user.publicDuel"
            :duel="$store.state.user.publicDuel"
        />
        <f7-button outline color="white" class="shuffle-btn">
            Перемешать
        </f7-button>
        <div class="empty"/>
    </div>
</template>

<script>
import ChallengeLine from '../components/ChallengeLine.vue';
import PublicChallenge from '../components/PublicChallenge.vue';

export default {
    name: 'TabChallenge',
    components: {
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

    .empty {
        height: 30px;
        width: 1px;
    }
</style>
