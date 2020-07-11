export default {
    setTab(state, tabIndex) {
        state.currentTab = tabIndex;
    },
    setLoading(state, isLoading) {
        state.isLoading = isLoading;
    },
    setMyDuels(state, myDuels) {
        state.myDuels = myDuels;
    },
    setPantheon(state, pantheon) {
        state.pantheon = pantheon;
    },
    setCategories(state, categories) {
        state.categories = categories;
    },
    setUser(state, user) {
        state.user = user;
    },
    setNotifications(state, notifications) {
        state.notifications = notifications;
    },
};
