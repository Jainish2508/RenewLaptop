/******************************************
    File Name: custom.js
*******************************************/

"use strict";

/**== wow animation ==**/

new WOW().init();

/**== loader js ==*/

$(window).on('load', function () {
    $(".bg_load").fadeOut("slow");
})

/**== Menu js ==**/

$("#navbar_menu").menumaker({
    title: "Menu",
    format: "multitoggle"
});

/** counter js **/

$("#counter").scroll(function () {
    $('.counter-count').each(function () {
        $(this).prop('Counter', 0).animate({
            Counter: $(this).text()
        }, {
            duration: 5000,
            easing: 'swing',
            step: function (now) {
                $(this).text(Math.ceil(now));
            }
        });
    });
});

/** progress_bar js **/

$(document).ready(function () {
    $('.progress .progress-bar').css("width",
        function () {
            return $(this).attr("aria-valuenow") + "%";
        }
    )
});

/** Casestudies Tab_bar js **/

$(document).ready(function () {
    $(".filter-button").click(function () {
        var value = $(this).attr('data-filter');

        if (value == "all") {
            //$('.filter').removeClass('hidden');
            $('.filter').show('1000');
        }
        else {
            //            $('.filter[filter-item="'+value+'"]').removeClass('hidden');
            //            $(".filter").not('.filter[filter-item="'+value+'"]').addClass('hidden');
            $(".filter").not('.' + value).hide('3000');
            $('.filter').filter('.' + value).show('3000');
        }
    });

    if ($(".filter-button").removeClass("active")) {
        $(this).removeClass("active");
    }
    $(this).addClass("active");
});

/** google_map js **/

function myMap() {
    var mapProp = {
        center: new google.maps.LatLng(40.712775, -74.005973),
        zoom: 18,
    };
    var map = new google.maps.Map(document.getElementById("googleMap"), mapProp);
}

/**===== Slider =====**/

var tpj = jQuery;
var revapi4;
tpj(document).ready(function () {
    if (tpj("#rev_slider_4_1").revolution == undefined) {
        revslider_showDoubleJqueryError("#rev_slider_4_1");
    } else {
        revapi4 = tpj("#rev_slider_4_1").show().revolution({
            sliderType: "standard",
            jsFileLocation: "revolution/js/",
            sliderLayout: "fullwidth",
            dottedOverlay: "none",
            delay: 7000,
            navigation: {
                keyboardNavigation: "off",
                keyboard_direction: "horizontal",
                mouseScrollNavigation: "off",
                onHoverStop: "off",
                touch: {
                    touchenabled: "on",
                    swipe_threshold: 75,
                    swipe_min_touches: 1,
                    swipe_direction: "horizontal",
                    drag_block_vertical: false
                },
                arrows: {
                    style: "zeus",
                    enable: true,
                    hide_onmobile: true,
                    hide_under: 600,
                    hide_onleave: true,
                    hide_delay: 200,
                    hide_delay_mobile: 1200,
                    tmp: '<div class="tp-title-wrap"><div class="tp-arr-imgholder"></div></div>',
                    left: {
                        h_align: "left",
                        v_align: "center",
                        h_offset: 30,
                        v_offset: 0
                    },
                    right: {
                        h_align: "right",
                        v_align: "center",
                        h_offset: 30,
                        v_offset: 0
                    }
                },
                bullets: {
                    enable: true,
                    hide_onmobile: true,
                    hide_under: 600,
                    style: "metis",
                    hide_onleave: true,
                    hide_delay: 200,
                    hide_delay_mobile: 1200,
                    direction: "horizontal",
                    h_align: "center",
                    v_align: "bottom",
                    h_offset: 0,
                    v_offset: 30,
                    space: 5,
                    tmp: '<span class="tp-bullet-img-wrap">  <span class="tp-bullet-image"></span></span><span class="tp-bullet-title">{{title}}</span>'
                }
            },
            viewPort: {
                enable: true,
                outof: "pause",
                visible_area: "80%"
            },
            responsiveLevels: [1240, 1024, 778, 480],
            gridwidth: [1240, 1024, 778, 480],
            gridheight: [700, 700, 500, 400],
            lazyType: "none",
            parallax: {
                type: "mouse",
                origo: "slidercenter",
                speed: 2000,
                levels: [2, 3, 4, 5, 6, 7, 12, 16, 10, 50],
            },
            shadow: 0,
            spinner: "off",
            stopLoop: "off",
            stopAfterLoops: -1,
            stopAtSlide: -1,
            shuffle: "off",
            autoHeight: "off",
            hideThumbsOnMobile: "off",
            hideSliderAtLimit: 0,
            hideCaptionAtLimit: 0,
            hideAllCaptionAtLilmit: 0,
            debugMode: false,
            fallbacks: {
                simplifyAll: "off",
                nextSlideOnWindowFocus: "off",
                disableFocusListener: false,
            }
        });
    }
});

/**===== End slider =====**/

/** header fixed js **/

$(window).scroll(function () {
    if ($(this).scrollTop() > 150) {
        $('.header_fixed_on_scroll').addClass('fixed-header');
    }
    else {
        $('.header_fixed_on_scroll').removeClass('fixed-header');
    }
});

/** contact form **/
(function ($) {
    "use strict";
    var spinner = $('#process-loader');
    $(document).ajaxStart(function () {
        spinner.show();
    });
    $(document).ajaxStop(function () {
        spinner.hide();
    });
    $("#contact_form").on("submit", function (event) {
        if (event.isDefaultPrevented()) {
            // handle the invalid form...
            formError();
            submitMSG(false, "Did you fill in the form properly?");
        } else {
            // everything looks good!
            event.preventDefault();
            submitForm();
        }
    });

    function submitForm() {
        let valdata = {};
        var form = $("#contact_form");
        var antiForgeryToken = $("input[name=__RequestVerificationToken]", form).val();
        valdata = {
            __RequestVerificationToken: antiForgeryToken,
            name: $("#name").val(),
            email: $("#email").val(),
            phone: $("#phone").val(),
            subject: $("#subject").val(),
            message: $("#message").val(),
            recaptcha: grecaptcha.getResponse()
        };

        $.ajax({
            async: true,
            type: 'POST',
            url: "Contact/Submit",
            data: valdata,
            contentType: "application/x-www-form-urlencoded",
            success: function (text) {
                if (text) {
                    formSuccess("#contact_form");
                    $('#submitbtn').prop("disabled", true);
                    $('#submitbtn').addClass("btn-disabled");
                    grecaptcha.reset();
                    location.reload();
                }
                else {
                    formError("#contact_form");
                    grecaptcha.reset();
                    submitMSG(false, "Please try again!");
                }
            }
            //error: function (XHR, textStatus, errorThrown) {
            //    alert(errorThrown);
            //}
        });
    }

    function formSuccess() {
        $("#contact_form")[0].reset();
        submitMSG(true, "Message Submitted!")
    }

    function formError() {
        $("#contact_form").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
            $(this).removeClass();
        });
    }

    function submitMSG(valid, msg) {
        if (valid) {
            var msgClasses = "h4 tada animated text-success center pt-30";
        } else {
            var msgClasses = "h4 text-danger center pt-30";
        }
        $("#msgSubmit").removeClass().addClass(msgClasses).text(msg);
    }
}(jQuery));

/** order form **/
(function ($) {
    "use strict";
    var spinner = $('#process-loader');
    $(document).ajaxStart(function () {
        spinner.show();
    });
    $(document).ajaxStop(function () {
        spinner.hide();
    });
    $("").on("submit", function (event) {
        if (event.isDefaultPrevented()) {
            formError();
            submitMSG(false, "Did you fill in the form properly?");
        }
        else {
            event.preventDefault();
            submitForm();
        }
    });

    function submitForm() {
        let valdata = {};
        var form = $("#checkoutForm");
        var antiForgeryToken = $("input[name=__RequestVerificationToken]", form).val();
        valdata = {
            __RequestVerificationToken: antiForgeryToken,
            FirstName: $("#fname").val(),
            LastName: $("#lname").val(),
            Email: $("#email").val(),
            Phone: $("#phone").val(),
            CompanyName: $("#company").val(),
            GSTIN: $("#gst").val(),
            AddressLine1: $("#addressline1").val(),
            AddressLine2: $("#addressline2").val(),
            City: $("#city").val(),
            State: $("#state").val(),
            PostalCode: $("#postalcode").val(),
            recaptcha: grecaptcha.getResponse()
        };
        let searchParams = new URLSearchParams(window.location.search);
        if (searchParams.has("product_id")) var param = parseInt(searchParams.get("product_id"));
        if (param > 1) {
            $("#modal").dialog({
                autoOpen: false,
                modal: true
            });
            $.ajax({
                async: true,
                type: 'POST',
                url: 'checkout?product_id='+param,
                data: valdata,
                contentType: "application/x-www-form-urlencoded",
                success: function () {
                    $('#submitbtn').prop("disabled", true);
                    $('#submitbtn').addClass("btn-disabled");
                }
                //error: function (XHR, textStatus, errorThrown) {
                //    alert(errorThrown);
                //}
            });
        }
        else {
            $.ajax({
                async: true,
                type: 'POST',
                url: window.location.pathname,
                data: valdata,
                contentType: "application/x-www-form-urlencoded",
                success: function (text) {
                    if (text) {
                        formSuccess();
                        $('#submitbtn').prop("disabled", true);
                        $('#submitbtn').addClass("btn-disabled");
                        grecaptcha.reset();
                        location.reload();
                    }
                    else {
                        formError();
                        grecaptcha.reset();
                        submitMSG(false, "Please try again!");
                    }
                }
                //error: function (XHR, textStatus, errorThrown) {
                //    alert(errorThrown);
                //}
            });
        }
    }

    function formSuccess() {
        $("#checkoutForm")[0].reset();
        submitMSG(true, "Subcription is started! You'll receive an email with receipt of your purchase.")
    }

    function formError() {
        $("#checkoutForm").removeClass().addClass('shake animated').one('webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend', function () {
            $(this).removeClass();
        });
    }

    function submitMSG(valid, msg) {
        if (valid) {
            var msgClasses = "h4 tada animated text-success center pt-30";
        } else {
            var msgClasses = "h4 text-danger center pt-30";
        }
        $("#msgSubmit").removeClass().addClass(msgClasses).text(msg);
    }
}(jQuery));