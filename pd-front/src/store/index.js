import Vue from 'vue';
import Vuex from 'vuex';
import mutations from './mutations';
import actions from './actions';
import getters from './getters';

Vue.use(Vuex);

export default new Vuex.Store({
    state: {
        myDuels: [],
        pantheon: [],
        categories: [],
        user: {},
        currentTab: 1,
        isLoading: false,
        notifications: true,
        noInternet: false,
        showOnboarding: false,
        lastDuelHash: '',
        currentDialog: null,
        lastApiError: '',
        lastStoryDuelId: '',
    },
    getters,
    mutations,
    actions,
});
