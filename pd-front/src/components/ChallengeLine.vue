<template>
    <div class="ch-line challenge-line-transition no-flicker" :style="`--translate: ${translate};`">
        <challenge
            class="challenge-block no-flicker"
            v-for="c in allChallenges"
            :key="c.id"
            :challenge-id="c.id"
            @click.native="$emit('go')"
        />
    </div>
</template>

<script>
import Challenge from './Challenge.vue';

export default {
    name: 'ChallengeLine',
    computed: {
        categoryId() {
            return this.$store.getters.challengeById(this.challengeId).categoryId;
        },
        category() {
            return this.$store.state.categories.find((c) => c.id === this.categoryId);
        },
        allChallenges() {
            return [{ id: -1 }, ...this.category.challenges, { id: -2 }];
        },
        challengeIndex() {
            return this.allChallenges.findIndex((c) => c.id === this.challengeId);
        },
        translate() {
            return `calc(-${this.challengeIndex * 100}vw)`;
        },
    },
    props: {
        challengeId: {
            type: Number,
            required: true,
        },
    },
    components: {
        Challenge,
    },
};
</script>

<style>
    .challenge-line-transition {
        transition-property: transform;
        transition-duration: 3s;
        transition-timing-function: cubic-bezier(0.08, 1.0, 0.51, 1.0);
    }
</style>

<style scoped>
    .ch-line {
        display: flex;
        width: 100vw;
        overflow: visible;
        /*noinspection CssUnresolvedCustomProperty*/
        transform: translateX(var(--translate)) translate3d(0, 0, 0);
    }

    .challenge-block {
        width: calc(100vw - 34px);
        min-width: calc(100vw - 34px);
        margin: 0 10px;
    }
</style>
