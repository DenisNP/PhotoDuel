<template>
    <div class="challenge">
        <div class="category">{{challenge.category.toLowerCase()}}</div>
        <div class="icon">
            <f7-icon
                class="icon-inner"
                :material="challenge.icon"
                :style="`color: ${challenge.color};`"
            />
        </div>
        <div class="text">
            <div class="name">
                {{challenge.name}}
            </div>
            <div class="description" :class="{'small-text': isLongDescription}">
                {{challenge.description}}
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name: 'Challenge',
    computed: {
        challenge() {
            return this.$store.getters.challengeById(this.challengeId);
        },
        isLongDescription() {
            return this.challenge && this.challenge.description.length > 60;
        },
    },
    props: {
        challengeId: {
            type: Number,
            required: true,
        },
    },
};
</script>

<style scoped>
    .challenge {
        background: white;
        border-radius: 10px;
        padding: 7px 7px 9px 7px;
        display: flex;
        align-items: center;
        position: relative;
        box-shadow: 0 1px 4px 0 rgba(0,0,0,0.35);
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

    .category {
        position: absolute;
        bottom: 1px;
        right: 4px;
        color: #aaa;
        font-size: 12px;
    }

    .icon-inner {
        font-size: 50px;
        width: 50px;
        height: 50px;
    }
</style>
