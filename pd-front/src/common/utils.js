export const isDev = () => process.env.NODE_ENV === 'development';

export const getSearch = () => {
    const { search } = window.location;
    return new URLSearchParams(search ? search.slice(1) : '');
};

export const getUserId = () => (isDev() ? '463377' : getSearch().get('vk_user_id'));

export const getAppId = () => (isDev() ? '7402641' : getSearch().get('vk_app_id'));

export const getHash = () => {
    const { hash } = window.location;
    return hash ? hash.slice(1) : '';
};

export const drawImage = (imageSrc, x, y, w, h) => new Promise((resolve) => {
    const img = new Image();
    img.crossOrigin = 'anonymous';
    img.onload = () => {
        const canvas = document.getElementById('canvas');
        const ctx = canvas.getContext('2d');
        if (w && h) ctx.drawImage(img, x, y, w, h);
        else ctx.drawImage(img, x, y);
        resolve();
    };
    img.src = imageSrc;
});
