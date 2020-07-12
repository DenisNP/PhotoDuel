import html2canvas from 'html2canvas';
import { drawImage } from './utils';
import soloBg from '../assets/story_single.jpg';
import voteBg from '../assets/story_bg.jpg';

const cWidth = 830;
const cHeight = 142;
const iSize = 850;
const ivSize = 750;

const createSoloCanvas = async (imageUrl, challengeElement) => {
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    await drawImage(soloBg, 0, 0);
    await drawImage(imageUrl, (1000 - iSize) / 2, 870 - iSize / 2, iSize, iSize);

    const challenge = await html2canvas(
        challengeElement,
        {
            width: 335,
            height: 57,
            scale: cWidth / 335.0,
        },
    );

    ctx.drawImage(challenge, (1000 - cWidth) / 2, 266 - cHeight / 2);
    return canvas.toDataURL('image/jpeg');
};

export const createSoloStory = async (imageUrl, challengeElement, duelId) => {
    console.log(duelId);
    console.log(await createSoloCanvas(imageUrl, challengeElement));
};

const createVoteCanvas = async (firstImgUrl, secondImgUrl, challengeElement) => {
    const canvas = document.getElementById('canvas');
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    await drawImage(voteBg, 0, 0);
    await drawImage(firstImgUrl, (1000 - ivSize) / 2, 430 - ivSize / 2, ivSize, ivSize);
    await drawImage(secondImgUrl, (1000 - ivSize) / 2, 1470 - ivSize / 2, ivSize, ivSize);

    const challenge = await html2canvas(
        challengeElement,
        {
            width: 335,
            height: 57,
            scale: cWidth / 335.0,
        },
    );

    ctx.drawImage(challenge, (1000 - cWidth) / 2, 970 - cHeight / 2);
    return canvas.toDataURL('image/jpeg');
};

export const createVoteStory = async (firstImgUrl, secondImgUrl, challengeElement, duelId) => {
    console.log(duelId);
    console.log(await createVoteCanvas(firstImgUrl, secondImgUrl, challengeElement));
};
