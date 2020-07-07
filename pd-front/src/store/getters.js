export default {
    challengeById(state) {
        const challenges = {};
        state.categories.forEach((cat) => {
            cat.challenges.forEach((c) => {
                challenges[c.id] = { ...c, category: cat.name, color: cat.color };
            });
        });
        return (id) => challenges[id];
    },
};
