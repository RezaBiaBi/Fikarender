new Swiper('#main-slider', {
    spaceBetween: 30,
    effect: 'fade',
    preloadImages: false,
    loop: true,
    autoplay: {
        delay: 5000,
        disableOnInteraction: false,
    },
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
        dynamicBullets: true,
    },
    navigation: {
        nextEl: '.swiper-button-next',
        prevEl: '.swiper-button-prev',
    },

});
var Berakpoints1 = {
    1300: {
        slidesPerGroup: 4,
        slidesPerView: 5,
        spaceBetween: 35,
    },
    1200: {
        slidesPerGroup: 4,
        slidesPerView: 5,
        spaceBetween: 25,
    },
    960: {
        slidesPerGroup: 3,
        slidesPerView: 4,
        spaceBetween: 30,
    },
    640: {
        slidesPerGroup: 2,
        slidesPerView: 3,
        spaceBetween: 25,
    },
    550: {
        speed: 300,
        slidesPerGroup: 2,
        slidesPerView: 2,
        spaceBetween: 60,
    },
    420: {
        speed: 300,
        slidesPerGroup: 2,
        slidesPerView: 2,
        spaceBetween: 15,
    },
    320: {
        speed: 300,
        slidesPerGroup: 2,
        slidesPerView: 2,
        spaceBetween: 10,
    }
}
new Swiper('#special-offer .swiper-container', {
    speed: 800,
    lazy: true,
    loop: false,
    centeredSlides: false,
    grabCursor: true,
    breakpoints: Berakpoints1,
    navigation: {
        nextEl: '#special-offer .swiper-button-prev',
        prevEl: '#special-offer .swiper-button-next',
    }
});
new Swiper('#most-sales .swiper-container', {
    speed: 800,
    lazy: true,
    loop: false,
    centeredSlides: false,
    grabCursor: true,
    breakpoints: Berakpoints1,
    navigation: {
        nextEl: '#most-sales .swiper-button-next',
        prevEl: '#most-sales .swiper-button-prev',
    }
});
new Swiper('#popular .swiper-container', {
    speed: 800,
    lazy: true,
    loop: false,
    centeredSlides: false,
    grabCursor: true,
    breakpoints: Berakpoints1,
    navigation: {
        nextEl: '#popular .swiper-button-next',
        prevEl: '#popular .swiper-button-prev',
    }
});