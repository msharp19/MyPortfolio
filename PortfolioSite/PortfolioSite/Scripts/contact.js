﻿$(function () {

    var isSubmittingOrderForm = false;

    $("#send-email").on("click", function () {
        if (isSubmittingOrderForm === false) {
            isSubmittingOrderForm === true;
            var isClean = checkForEmptyFields();
            var name = $("#name-val").val();
            var email = $("#email-val").val();
            var comment = $("#comments").text();
            var data = { name: name, email: email, message: comment, CaptchaDeText: $("#CaptchaDeText").val(), CaptchaInputText: $("#CaptchaInputText").val() };
            if (isClean === true) {
                toggle();
                $.ajax({
                    type: "POST",
                    url: "../Contact/Message",
                    data: data,
                    success: function (data) {
                        if (data.Status === "Success") {
                            outputSuccessMessage(data.Message, "Contact Recieved");
                            setTimeout(function () {
                                window.location.assign(data.Url);
                            }, 1200);
                        }
                        else {
                            outputErrorMessage(data.Message, "Captcha Failure");
                            setTimeout(function () {
                                window.location.assign(data.Url);
                            }, 1200);
                        }
                        toggle();
                    },
                    error: function () {
                        outputErrorMessage("Something went wrong :/", "Opps");
                        isSubmittingOrderForm = false;
                        toggle();
                    }
                });
            } else {
                isSubmittingOrderForm === false;
                outputWarningMessage("Please fill in all fields on form", "Missing Data");
            }
        } else {
            outputWarningMessage("Please wait until this message has sent", "Sending..");
        }
    });

    function toggle() {
        $("#email-load").toggleClass("none");
        $("#email-load").toggleClass("inline");
    }

});