<template>
    <f7-page>
        <f7-navbar :title="title"/>
        <f7-toolbar tabbar labels position="bottom">
            <f7-link
                tab-link="#tab-pantheon"
                text="Пантеон"
                icon-material="leaderboard"
            />
            <f7-link
                tab-link="#tab-challenge"
                tab-link-active
                text="Вызов"
                icon-material="camera"
            />
            <f7-link
                tab-link="#tab-duels"
                text="Дуэли"
                icon-material="view_day"
            />
        </f7-toolbar>
        <f7-tabs class="full-height">
            <f7-tab id="tab-pantheon" @tab:show="changeTitle('Работы победителей')">
                <tab-pantheon/>
            </f7-tab>
            <f7-tab
                id="tab-challenge"
                class="full-height"
                tab-active @tab:show="changeTitle('Фотодуэль')"
            >
                <tab-challenge/>
            </f7-tab>
            <f7-tab id="tab-duels" @tab:show="changeTitle('Ваши дуэли')">
                <tab-duels/>
            </f7-tab>
        </f7-tabs>
    </f7-page>
</template>

<script>
import TabChallenge from './TabChallenge.vue';
import TabPantheon from './TabPantheon.vue';
import TabDuels from './TabDuels.vue';

export default {
    name: 'Main',
    data() {
        return {
            title: 'Фотодуэль',
        };
    },
    methods: {
        changeTitle(t) {
            this.title = t;
        },
    },
    components: {
        TabChallenge,
        TabPantheon,
        TabDuels,
    },
};
</script>

<style>
    .full-height {
        height: 100%;
    }
</style>
