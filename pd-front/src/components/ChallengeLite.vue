<template>
    <div class="challenge">
        <div class="icon" v-if="!noIcon">
            <f7-icon :size="shrank ? 25 : 40" :material="challenge.icon"/>
        </div>
        <div class="text">
            <div class="name">
                {{challenge.name}}
            </div>
            <div class="description" :class="{'small-text': isLongDescription}" v-if="!shrank">
                {{challenge.description}}
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'ChallengeLite',
    computed: {
        challenge() {
            return this.$store.getters.challengeById(this.challengeId);
        },
        isLongDescription() {
            return this.challenge.description.length > 60;
        },
    },
    props: {
        challengeId: {
            type: Number,
            required: true,
        },
        noIcon: {
            type: Boolean,
            default: false,
        },
        shrank: {
            type: Boolean,
            default: false,
        },
    },
};
</script>

<style scoped>
    .challenge {
        padding: 5px;
        display: flex;
        align-items: center;
        position: relative;
    }

    .icon {
        margin-right: 5px;
    }

    .text {
        margin-bottom: 3px;
    }

    .name {
        font-size: 14px;
        font-weight: bold;
    }

    .description {
        font-size: 13px;
    }

    .small-text {
        font-size: 11px;
    }
</style>
