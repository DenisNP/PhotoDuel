import VKC from '@denisnp/vkui-connect-helper';
import api from '../common/api';
import {
    getAppId, getHash, getSearch, isDev, getPlatform,
} from '../common/utils';

export default {
    async api({ commit }, { method, data, disableLoading }) {
        if (!disableLoading) commit('setLoading', true);
        const result = await api(method, data);
        commit('setNoInternet', !result);
        if (result && result.error) {
            commit('setLastApiError', result.error);
        }
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
            if (hash.length >= 8 && !hash.startsWith('!')) {
                [data.duelId] = hashData;
                commit('setLastDuelHash', data.duelId);
            }
        }
        // notifications
        const notifications = Number.parseInt(getSearch().get('vk_are_notifications_enabled'), 10) !== 0;
        commit('setNotifications', notifications);
    },

    async init({ state, commit, dispatch }, background) {
        const data = {
            vote: 'None',
            duelId: state.lastDuelHash,
        };
        if (!background) {
            VKC.init({
                appId: getAppId(),
                accessToken: getPlatform() === 'local' ? process.env.VUE_APP_VK_DEV_TOKEN : '',
                asyncStyle: true,
                uploadProxy: isDev() ? 'http://localhost:5000/proxy' : '/proxy',
                apiVersion: '5.120',
            });

            // set bar color
            VKC.bridge().send(
                'VKWebAppSetViewSettings',
                { status_bar_style: 'light', action_bar_color: '#EB643A' },
            );

            // load
            dispatch('load', data);
            // reload on restore from cache
            VKC.subscribe((evt) => {
                if (!evt.detail) return;
                if (evt.detail.type === 'VKWebAppViewRestore') {
                    commit('setLoading', false);
                    dispatch('init', false);
                } else if (evt.detail.type === 'VKWebAppShowStoryBoxLoadFinish') {
                    try {
                        const duelId = state.lastStoryDuelId;
                        if (!duelId) return;
                        const d = evt.detail.data;
                        dispatch('api', {
                            method: 'updateStory',
                            data: {
                                duelId,
                                storyUrl: `https://vk.com/story${d.story_owner_id}_${d.story_id}`,
                            },
                            disableLoading: true,
                        }); // TODO update duel object
                    } catch (e) {
                        console.log('Error story loading', e, evt);
                    }
                }
            });

            // onboarded
            const [onb] = await VKC.send('VKWebAppStorageGet', { keys: ['onboarded'] });
            if (onb && onb.keys) {
                if (!onb.keys.some((k) => k.key === 'onboarded' && k.value)) {
                    commit('setShowOnboarding', true);
                }
            }
        }

        const result = await dispatch('api', { method: 'init', data });
        if (!result) {
            return {
                requestNotify: false,
                message: '',
            };
        }

        if (result.categories) commit('setCategories', result.categories);
        if (result.myDuels) commit('setMyDuels', result.myDuels);
        if (result.pantheon) commit('setPantheon', result.pantheon);
        if (result.user) commit('setUser', result.user);

        let requestNotify = false;
        if (result.voted) {
            commit('setTab', 2);
            if (!state.notifications) requestNotify = true;
        }

        return {
            requestNotify,
            message: result.message || '',
        };
    },

    async shuffle({ commit, dispatch }) {
        const userResponse = await dispatch('api', { method: 'shuffle', data: {} });
        if (userResponse && userResponse.user) commit('setUser', userResponse.user);
    },

    async createDuel({ commit, dispatch }, { challengeId, duelId, file }) {
        commit('setLoading', true);
        // TODO write challenge name as caption
        const [photo] = await VKC.uploadWallPhoto(file, '', '', 'photos');
        let url = '';
        if (photo && photo.response && photo.response[0]) {
            const photoSize = photo.response[0].sizes.find((s) => s.type === 'y');
            url = (photoSize && photoSize.url) || '';
        }
        if (!url) {
            commit('setLoading', false);
            return false;
        }

        const data = {
            challengeId,
            duelId,
            image: url,
            photoId: '',
        };

        const duelResponse = await dispatch('api', { method: duelId ? 'join' : 'create', data });
        if (!duelResponse || !duelResponse.duel) {
            commit('setLoading', false);
            return false;
        }

        commit('setNewMyDuel', duelResponse.duel);
        commit('setLoading', false);
        return true;
    },

    async publish({ commit, dispatch }, duelId) {
        const duelResponse = await dispatch('api', { method: 'publish', data: { duelId } });
        if (duelResponse && duelResponse.duel) {
            commit('setNewMyDuel', duelResponse.duel);
            return true;
        }
        return false;
    },

    async deleteDuel({ commit, dispatch }, duelId) {
        const okResponse = await dispatch('api', { method: 'delete', data: { duelId } });
        if (okResponse && okResponse.ok) {
            commit('deleteDuel', duelId);
            return true;
        }
        return false;
    },

    saveOnboarding({ commit }) {
        commit('setShowOnboarding', false);
        VKC.send('VKWebAppStorageSet', { key: 'onboarded', value: '1' });
    },

    openDialog({ commit }, dialog) {
        commit('setCurrentDialog', dialog);
        if (window.history.state && window.history.state.view_main && window.history.state.view_main.url === '/') {
            window.history.pushState('dialog', null);
        }
    },

    closeDialog({ state, commit }) {
        if (state.currentDialog && state.currentDialog.opened) {
            state.currentDialog.close();
            commit('setCurrentDialog', null);
        }
    },
};
