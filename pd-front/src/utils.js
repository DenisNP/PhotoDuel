export const isDev = () => process.env.NODE_ENV === 'development';

export const getSearch = () => {
    const { search } = window.location;
    return new URLSearchParams(search ? search.slice(1) : '');
};

export const getHash = () => {
    const { hash } = window.location;
    return hash ? hash.slice(1) : '';
};
