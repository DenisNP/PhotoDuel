export default {
    challengeById(state) {
        const challenges = {};
        state.categories.forEach((cat) => {
            cat.challenges.forEach((c) => {
                challenges[c.id] = {
                    ...c,
                    category: cat.name,
                    categoryId: cat.id,
                    color: cat.color,
                };
            });
        });
        return (id) => challenges[id];
    },

    currentDuels(state) {
        return state.myDuels.filter(
            (d) => d.status !== 'Finished'
                && (
                    d.creator.user.id === state.user.id
                    || (
                        d.opponent !== null
                        && d.opponent.user.id === state.user.id
                    )
                ),
        );
    },

    hasCurrentDuels(_, getters) {
        return getters.currentDuels.length > 0;
    },
};
