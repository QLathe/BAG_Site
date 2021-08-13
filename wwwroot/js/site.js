// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/* When the user clicks on the button, 
toggle between hiding and showing the dropdown content */

function myFunction() {
    // This function is what makes the dropdown menu work
    document.getElementById("myDropdown").classList.toggle("show");
}

// Close the dropdown if the user clicks outside of it
window.onclick = function (e) {
    if (!e.target.matches('.dropbtn')) {
        var myDropdown = document.getElementById("myDropdown");
        if (myDropdown.classList.contains('show')) {
            myDropdown.classList.remove('show');
        }
    }
}

//feature slides coding
var slideIndex = 1;
showSlides(slideIndex);

// Next/previous controls
function plusSlides(n) {
    $('mySlides, .activef').fadeOut();
    setTimeout(function() {
        showSlides(slideIndex += n);
    }, 500);
}


// Thumbnail image controls
function currentSlide(n) {
    $('mySlides, .activef').fadeOut();
    setTimeout(function() {
        showSlides(slideIndex = n);
    }, 500);
}

function showSlides(n) {
    var i;
    var slides = document.getElementsByClassName("mySlides");
    var dots = document.getElementsByClassName("dot");
    if (n > slides.length) { slideIndex = 1 }
    if (n < 1) { slideIndex = slides.length }
    for (i = 0; i < slides.length; i++) {
        slides[i].style.display = "none";
        slides[i].className = slides[i].className.replace( " activef", "");
    }
    for (i = 0; i < dots.length; i++) {
        dots[i].className = dots[i].className.replace(" active", "");
    }
    $(slides[slideIndex - 1]).fadeIn();
    slides[slideIndex - 1].className += " activef";
    dots[slideIndex - 1].className += " active";
}

document.addEventListener("DOMContentLoaded", function () {
    var lazyloadImages = document.querySelectorAll("img.lazy");
    var lazyloadThrottleTimeout;

    function lazyload() {
        if (lazyloadThrottleTimeout) {
            clearTimeout(lazyloadThrottleTimeout);
        }

        lazyloadThrottleTimeout = setTimeout(function () {
            var scrollTop = window.pageYOffset;
            lazyloadImages.forEach(function (img) {
                if (img.offsetTop < (window.innerHeight + scrollTop)) {
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                }
            });
            if (lazyloadImages.length == 0) {
                document.removeEventListener("scroll", lazyload);
                window.removeEventListener("resize", lazyload);
                window.removeEventListener("orientationChange", lazyload);
            }
        }, 20);
    }

    document.body.addEventListener("scroll", lazyload);
    window.addEventListener("resize", lazyload);
    window.addEventListener("orientationChange", lazyload);
});
// $(document).ready(function () {
//     $("#btnsubmit").click(function (e) {
//         //Serialize the form datas.   
//         var valdata = $("#commentForm").serialize();
//         $.ajax({
//             url: "/WriteComment",
//             type: "POST",
//             dataType: 'json',
//             contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
//             data: valdata
//         });
//     });
// });
