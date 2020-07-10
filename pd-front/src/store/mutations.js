export default {
    setTab(state, tabIndex) {
        state.currentTab = tabIndex;
    },
    setLoading(state, isLoading) {
        state.isLoading = isLoading;
    },
};
