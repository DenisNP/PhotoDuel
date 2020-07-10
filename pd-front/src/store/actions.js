import api from '../api';

export default {
    async api({ commit }, { method, data, disableLoading }) {
        if (!disableLoading) commit('setLoading', true);
        const result = await api(method, data);
        if (!disableLoading) commit('setLoading', false);
        return result;
    },
};
