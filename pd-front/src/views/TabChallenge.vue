<template>
    <div class="tab-challenge">
        <duel :duel="$store.state.myDuels[0]"/>
    </div>
</template>

<script>
import Duel from '../components/Duel.vue';

export default {
    name: 'TabChallenge',
    components: {
        Duel,
    },
};
</script>

<style scoped>
    .tab-challenge {
        background-color: var(--f7-theme-color);
        height: 100%;
    }
</style>
