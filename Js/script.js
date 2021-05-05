var $, document, window;
$(document).ready(function () {
    $(".show_hide_password a").on('click', function (event) {
        event.preventDefault();
        if ($('.show_hide_password input').attr("type") == "text") {
            $('.show_hide_password input').attr('type', 'password');
            $('.show_hide_password i').addClass("fa-eye-slash");
            $('.show_hide_password i').removeClass("fa-eye");
        } else if ($('.show_hide_password input').attr("type") == "password") {
            $('.show_hide_password input').attr('type', 'text');
            $('.show_hide_password i').removeClass("fa-eye-slash");
            $('.show_hide_password i').addClass("fa-eye");
        }
    });
});
$(document).ready(function () {
    $(".show_hide_password1 a").on('click', function (event) {
        event.preventDefault();
        if ($('.show_hide_password1 input').attr("type") == "text") {
            $('.show_hide_password1 input').attr('type', 'password');
            $('.show_hide_password1 i').addClass("fa-eye-slash");
            $('.show_hide_password1 i').removeClass("fa-eye");
        } else if ($('.show_hide_password1 input').attr("type") == "password") {
            $('.show_hide_password1 input').attr('type', 'text');
            $('.show_hide_password1 i').removeClass("fa-eye-slash");
            $('.show_hide_password1 i').addClass("fa-eye");
        }
    });
});
$(document).ready(function () {
    $(".show_hide_password2 a").on('click', function (event) {
        event.preventDefault();
        if ($('.show_hide_password2 input').attr("type") == "text") {
            $('.show_hide_password2 input').attr('type', 'password');
            $('.show_hide_password2 i').addClass("fa-eye-slash");
            $('.show_hide_password2 i').removeClass("fa-eye");
        } else if ($('.show_hide_password2 input').attr("type") == "password") {
            $('.show_hide_password2 input').attr('type', 'text');
            $('.show_hide_password2 i').removeClass("fa-eye-slash");
            $('.show_hide_password2 i').addClass("fa-eye");
        }
    });
});
$(document).ready(function () {
    $(".show_hide_password3 a").on('click', function (event) {
        event.preventDefault();
        if ($('.show_hide_password3 input').attr("type") == "text") {
            $('.show_hide_password3 input').attr('type', 'password');
            $('.show_hide_password3 i').addClass("fa-eye-slash");
            $('.show_hide_password3 i').removeClass("fa-eye");
        } else if ($('.show_hide_password3 input').attr("type") == "password") {
            $('.show_hide_password3 input').attr('type', 'text');
            $('.show_hide_password3 i').removeClass("fa-eye-slash");
            $('.show_hide_password3 i').addClass("fa-eye");
        }
    });
});


$(function () {
    'use strict';
    $(window).scroll(function () {
        showhidenav();
    });


    function showhidenav() {
        if ($(window).scrollTop() > 50) {
            $("#nav").addClass("white-nav-top");
            $(".navbar-brand img").attr("src", "../Front_Images/images/logo.png");


        } else {
            $("#nav").removeClass("white-nav-top");
            $(".navbar-home img").attr("src", "../Front_Images/images/top-logo.png");
            
        }

    }
});


$(function () {
    
    'use strict';
    $("a.smooth-scroll").click(function (event) {
        event.preventDefault();
        var section_id = $(this).attr("href");

        $("html, body").animate({
            scrollTop: $(section_id).offset().top - 64
        }, 1250, "easeInOutExpo");
    });
});

$(function () {
    
    'use strict';
    $("#mobile-nav-open-btn").click(function () {
        $("#mobile-nav").css("height", "100%");
    });
    $("#mobile-nav-close-btn, #mobile-nav a").click(function () {
        $("#mobile-nav").css("height", "0%");
    });


});

document.querySelector(".button").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});

document.querySelector(".close").addEventListener("click", function() {
    document.querySelector(".popup").style.display = "none";
    $('body').css('overflow', '');
    document.querySelector(".nav-popup").style.background = "rgba(255,255,255)";
});

document.querySelector(".no-buu").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu1").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu2").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu3").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu4").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu5").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu6").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu7").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu8").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});
document.querySelector(".no-buu9").addEventListener("click", function(){
    document.querySelector(".popup").style.display = "flex";
    $('body').css('overflow', 'hidden');
    document.querySelector(".nav-popup").style.background = "rgba(0,0,0,0.0092)";
});














