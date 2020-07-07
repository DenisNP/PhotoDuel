<template>
    <div class="tab-challenge">
        <challenge-lite :challenge-id="1001"/>
    </div>
</template>

<script>
import ChallengeLite from '../components/ChallengeLite.vue';

export default {
    name: 'TabChallenge',
    components: {
        ChallengeLite,
    },
};
</script>

<style scoped>
    .tab-challenge {
        background-color: var(--f7-theme-color);
        height: 100%;
    }
</style>
