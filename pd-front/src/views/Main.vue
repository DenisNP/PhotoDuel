<template>
    <f7-page>
        <f7-navbar :title="title"/>
        <f7-toolbar tabbar labels position="bottom">
            <f7-link
                tab-link="#tab-pantheon"
                :tab-link-active="currentTab === 0"
                text="Пантеон"
                icon-material="leaderboard"
                @click="$store.commit('setTab', 0)"
            />
            <f7-link
                tab-link="#tab-challenge"
                :tab-link-active="currentTab === 1"
                text="Вызов"
                icon-material="camera"
                @click="$store.commit('setTab', 1)"
            />
            <f7-link
                tab-link="#tab-duels"
                :tab-link-active="currentTab === 2"
                text="Дуэли"
                icon-material="view_day"
                @click="$store.commit('setTab', 2)"
            />
        </f7-toolbar>
        <f7-tabs class="full-height" :class="{colorful: isMainTab, graybg: !isMainTab}">
            <f7-tab v-show="currentTab === 0" :tab-active="currentTab === 0">
                <tab-pantheon/>
            </f7-tab>
            <f7-tab
                v-show="currentTab === 1"
                class="full-height"
                :tab-active="currentTab === 1"
            >
                <tab-challenge/>
            </f7-tab>
            <f7-tab v-show="currentTab === 2" :tab-active="currentTab === 2" class="full-height">
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
    computed: {
        currentTab() {
            return this.$store.state.currentTab;
        },
        title() {
            switch (this.currentTab) {
            case 0:
                return 'Работы победителей';
            case 2:
                return 'Ваши дуэли';
            default:
                return 'Фотодуэль';
            }
        },
        isMainTab() {
            return this.currentTab === 1;
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
        min-height: 100%;
    }

    .colorful {
        background-color: var(--f7-theme-color);
    }

    .graybg {
        background-color: #eee;
    }
</style>
