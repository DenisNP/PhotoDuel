<template>
    <div class="ch-line" :style="`--translate: ${translate};`">
        <challenge
            class="challenge-block"
            v-for="c in category.challenges"
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
        challengeIndex() {
            return this.category.challenges.findIndex((c) => c.id === this.challengeId);
        },
        translate() {
            return `calc(${34 + 66 * this.challengeIndex}px - ${this.challengeIndex * 100}vw)`;
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

<style scoped>
    .ch-line {
        display: flex;
        width: 100vw;
        overflow: visible;
        /*noinspection CssUnresolvedCustomProperty*/
        transform: translateX(var(--translate));
        transition: transform 3s cubic-bezier(.08,1.05,.51,1);
    }

    .challenge-block {
        width: calc(100vw - 100px);
        min-width: calc(100vw - 100px);
        margin: 0 10px;
    }
</style>
