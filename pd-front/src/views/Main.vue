<template>
    <f7-page>
        <f7-navbar :title="title"/>
        <f7-toolbar tabbar labels position="bottom">
<!--            <f7-link-->
<!--                tab-link="#tab-pantheon"-->
<!--                :tab-link-active="currentTab === 0"-->
<!--                text="Пантеон"-->
<!--                icon-material="leaderboard"-->
<!--                @click="$store.commit('setTab', 0)"-->
<!--            />TODO-->
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
                <tab-challenge @reload="load"/>
            </f7-tab>
            <f7-tab v-show="currentTab === 2" :tab-active="currentTab === 2" class="full-height">
                <tab-duels/>
            </f7-tab>
        </f7-tabs>
    </f7-page>
</template>

<script>
import VKC from '@denisnp/vkui-connect-helper';
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
    methods: {
        async load() {
            const { message, requestNotify } = await this.$store.dispatch('init', false);
            if (requestNotify) {
                this.$f7.dialog.confirm(
                    'Ваш голос засчитан. Отправить вам уведомление с результатами, когда дуэль закончится?',
                    'Голосование',
                    () => VKC.send('VKWebAppAllowNotifications', {}),
                );
            } else if (message) {
                this.toast(message);
            }
        },
        toast(message) {
            this.$f7.toast.create({
                text: message,
                position: 'center',
                cssClass: 'my-text-center',
                closeTimeout: 2000,
            }).open();
        },
    },
    components: {
        TabChallenge,
        TabPantheon,
        TabDuels,
    },
    mounted() {
        this.load();
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
