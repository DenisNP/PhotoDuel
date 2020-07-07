<template>
    <div class="tab-challenge">
        <user :user="$store.state.user"/>
    </div>
</template>

<script>
import User from '../components/User.vue';

export default {
    name: 'TabChallenge',
    components: {
        User,
    },
};
</script>

<style scoped>
    .tab-challenge {
        background-color: var(--f7-theme-color);
        height: 100%;
    }
</style>
