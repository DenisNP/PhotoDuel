<template>
    <div class="public-challenge-container">
        <div class="category">{{categoryName}}</div>
        <div class="pc-photo">
            <img :src="duel.creator.image"/>
        </div>
        <div class="pc-data">
            <challenge-lite :challenge-id="duel.challengeId" :no-icon="true"/>
            <user class="pc-user" :user="duel.creator.user"/>
        </div>
    </div>
</template>

<script>
import ChallengeLite from './ChallengeLite.vue';
import User from './User.vue';

export default {
    name: 'PublicChallenge',
    computed: {
        categoryName() {
            return this.$store.getters.challengeById(this.duel.challengeId).category;
        },
    },
    props: {
        duel: {
            type: Object,
            required: true,
        },
    },
    components: {
        User,
        ChallengeLite,
    },
};
</script>

<style scoped>
    .public-challenge-container {
        display: flex;
        position: relative;
        background: white;
        border-radius: 10px;
        padding: 10px;
        align-items: center;
        box-shadow: 0 1px 4px 0 rgba(0,0,0,0.35);
    }

    .category {
        position: absolute;
        top: 1px;
        right: 4px;
        color: #aaa;
        font-size: 12px;
    }

    .pc-photo {
        width: 100px;
        min-width: 100px;
        height: 100px;
        border-radius: 5px;
        overflow: hidden;
    }

    .pc-photo > img {
        width: 100%;
        height: 100%;
    }

    .pc-data {
        margin-left: 10px;
        display: flex;
        min-height: 100px;
        flex-direction: column;
        justify-content: space-between;
    }

    .pc-user {
        margin-left: 5px;
        margin-bottom: 5px;
    }
</style>
