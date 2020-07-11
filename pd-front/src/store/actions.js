import VKC from '@denisnp/vkui-connect-helper';
import api from '../api';
import {
    getAppId, getHash, getSearch, isDev,
} from '../utils';

export default {
    async api({ commit }, { method, data, disableLoading }) {
        if (!disableLoading) commit('setLoading', true);
        const result = await api(method, data);
        if (!disableLoading) commit('setLoading', false);
        return result;
    },

    load({ commit }, inputData) {
        const data = inputData;
        const hash = getHash();
        if (hash) {
            const hashData = hash.split('_');
            if (hashData[1] && hashData[1] === 'vote') {
                if (hashData[2] === 'c') data.vote = 'Creator';
                if (hashData[2] === 'o') data.vote = 'Opponent';
            }
            [data.duelId] = hashData;
        }
        // notifications
        const notifications = Number.parseInt(getSearch().get('vk_are_notifications_enabled'), 10) !== 0;
        commit('setNotifications', notifications);
    },

    async init({ commit, dispatch }, background) {
        const data = {
            vote: 'None',
            duelId: '',
        };
        if (!background) {
            VKC.init({
                appId: getAppId(),
                accessToken: process.env.VK_DEV_TOKEN,
                asyncStyle: true,
                uploadProxy: isDev() ? 'http://localhost:5000/proxy' : '/proxy',
                apiVersion: '5.120',
            });
            dispatch('load', data);
        }

        const result = await dispatch('api', { method: 'init', data });
        if (!result) {
            // TODO no internet
            return '';
        }

        if (result.categories) commit('setCategories', result.categories);
        if (result.myDuels) commit('setMyDuels', result.myDuels);
        if (result.pantheon) commit('setPantheon', result.pantheon);
        if (result.user) commit('setUser', result.user);

        return result.message || '';
    },

    async shuffle({ commit, dispatch }) {
        const userResponse = await dispatch('api', { method: 'shuffle', data: {} });
        if (userResponse) commit('setUser', userResponse.user);
    },
};
