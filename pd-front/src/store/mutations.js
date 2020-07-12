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
    setNoInternet(state, noInternet) {
        state.noInternet = noInternet;
    },
    setShowOnboarding(state, showOnboarding) {
        state.showOnboarding = showOnboarding;
    },
    setNewMyDuel(state, duel) {
        const myDuel = state.myDuels.find((d) => d.id === duel.id);
        if (myDuel) {
            Object.keys(duel).forEach((k) => {
                myDuel[k] = duel[k];
            });
            return;
        }
        state.myDuels.unshift(duel);
    },
    deleteDuel(state, duelId) {
        const myDuelIdx = state.myDuels.findIndex((d) => d.id === duelId);
        if (myDuelIdx >= 0) {
            state.myDuels.splice(myDuelIdx, 1);
        }
    },
};
