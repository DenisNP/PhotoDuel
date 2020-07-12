const emptyChallenge = (id) => ({
    id,
    name: '...',
    description: '...',
    icon: 'crop_square',
    category: ' ',
    color: '#000000',
    categoryId: -1,
});

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
        return (id) => challenges[id] || emptyChallenge(id);
    },

    currentDuels(state) {
        return state.myDuels.filter(
            (d) => {
                const isMine = d.creator.user.id === state.user.id;
                const iAmOpponent = d.opponent !== null && d.opponent.user.id === state.user.id;
                const createdNonPublic = d.status === 'Created' && !d.isPublic;
                const participateIn = isMine || iAmOpponent;
                const participateNow = participateIn && d.status !== 'Finished';

                return participateNow || createdNonPublic;
            },
        );
    },

    hasCurrentDuels(_, getters) {
        return getters.currentDuels.length > 0;
    },
};
