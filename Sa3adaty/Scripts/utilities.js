var valid_div = "<div class='valid_div'></div>";
var invalid_div = "<div class='invalid_div'></div>";

function checkIfNotBlank(input) {
    if ($(input).val().replace(/^\s+|\s+$/g, "").length != 0) { $(input).removeClass("required"); return true; }
    else {
        $(input).addClass("required");
        return false
    };
}


//main menu events
$(document).ready()
{
    $(".menu_login").click(function (event) {
        if ($(this).hasClass("menu_login")) {
            if ($(this).hasClass('active'))
            {
                $(this).removeClass('active');
            }
            else
            {
                $(this).addClass('active');
                $('html').one('click', function () {
                    $(".menu_login").removeClass('active');
                });
                event.stopPropagation();
            }
        }
    });
    //Disable children events
    $(".menu_login *").click(function (e) {
        e.stopPropagation();
    });

    $(".menu_profile").click(function (event) {
        if ($(this).hasClass('active')) {
            $(this).removeClass('active');
        }
        else {
            $(this).addClass('active');
            $('html').one('click', function () {
                $(".menu_profile").removeClass('active');
            });
            event.stopPropagation();
        }
    });
   

    $("#header-password").focus(function () {
        if ($(this).hasClass("required")) {
            $(this).val("");
            $(this).removeClass("required");
            $(this).attr("type", "password");
            $("#header_password_div").each(".invalid_div").remove();
        }
    }).blur(function () {
        validatePassword(this);
    });

    $("#header-email").focus(function () {
        if ($(this).hasClass("required")) {
            $(this).val("");
            $(this).removeClass("required");
            $("#header_email_div").find(".invalid_div").remove();
        }
    }).blur(function () {
        validateLoginEmail(this, "#header_email_div");
    });
}

function validateLoginEmail(input,wrapper) {
    if (checkIfNotBlank(input)) {
        $.post("/Account/_IsAvailableEmail", { email: $(input).val() })
        .done(function (data) {
            if (data.isvalid == true) {
                if (data.isavailable) {
                    $(input).addClass("required");
                    $(wrapper).find(".valid_div").remove();
                    $(wrapper).append(invalid_div);
                    $(input).val("البريد الإلكتروني غير متوفر");
                             
                }
                else {
                    $(wrapper).find(".invalid_div").remove();
                    $(wrapper).append(valid_div);
                }
            }
            else {
                $(input).addClass("required");
                $(wrapper).find(".valid_div").remove();
                $(wrapper).append(invalid_div);
                $(input).val("البريد الإلكتروني خاطئ");
            }

            })
        .fail(function () {
        })
        .always(function () {
        });
    }
    else {
        $(wrapper).find(".valid_div").remove();
        $(input).val("الرجاء ادخل البريد اللإلكتروني");
        }
}

function validatePassword(input,wrapper) {
    if (checkIfNotBlank(input)) {
        $.post("/Account/_IsValidPassword", { password: $(input).val() })
        .done(function (data) {
            if (data.isvalid == true) {
                $(wrapper).append(valid_div);
            }
            else {
                $(input).addClass("required");
                $(wrapper).find(".valid_div").remove();
                $(input).attr("type", "text");
                $(input).val("كلمة السر من 6-20 خانة");
            }

        })
        .fail(function () {
        })
        .always(function () {
        });
    }
    else {
        $(wrapper).find(".valid_div").remove();
        $(input).attr("type", "text");
        $(input).val("الرجاء ادخل كلمة السر");
    }
}

function validateSignupEmail(input,wrapper) {
    if (checkIfNotBlank(input)) {
        $.post("/Account/_IsAvailableEmail", { email: $(input).val() })
        .done(function (data) {
            if (data.isvalid == true) {
                if (data.isavailable)
                {
                    $(wrapper).append(valid_div);
                }
                else
                {
                    $(input).addClass("required");
                    $(wrapper).find(".valid_div").remove();
                    $(input).val("البريد الإلكتروني غير متوفر");
                }
            }
            else {
                $(input).addClass("required");
                $(wrapper).find(".valid_div").remove();
                $(input).val("البريد الإلكتروني خاطئ");
            }

        })
        .fail(function () {
        })
        .always(function () {
        });
    }
    else {
            $(wrapper).find(".valid_div").remove();
            $(input).val("الرجاء ادخل البريد اللإلكتروني");
    }
}

function textCounter(field, field2, maxlimit) {
    var countfield = document.getElementById(field2);
    countfield.value = maxlimit - field.value.length;
}

function textCounterLimited(field, field2, maxlimit) {
    var countfield = document.getElementById(field2);
    if (field.value.length > maxlimit) {
        field.value = field.value.substring(0, maxlimit);
        return false;
    } else {
        countfield.value = maxlimit - field.value.length;
    }
}

/*Poll Functions*/
function show_correct() {
    $(".show_correct_label").hide();
    $(".show_correct_answer").show();
}
function poll_begin(poll_id) {
    if ($("#Poll_" + poll_id + " input[type='radio']:checked").length) {
        $("#poll-answer-required-" + poll_id).hide();
        $("#Poll_" + poll_id + " .poll-radio-wraper input[type=radio]").attr('disabled', true);
        $("#Poll_" + poll_id + " .poll-radio-wraper .action-btn").attr('disabled', true);
    }
    else {
        $("#poll-answer-required-" + poll_id).show();
        return false;

    }
}
function poll_success(data) {
    if (data.success == false) {
        $("#Poll_" + data.poll_id + " .poll-radio-wraper input[type=radio]").attr('disabled', false);
        $("#Poll_" + data.poll_id + " .poll-radio-wraper .action-btn").attr('disabled', false);
        alert("حدث خطأ أثناء إرسال طلبك، الرجاء المحاولة لاحقا");
    }
    else {
        $("#Poll_" + data.poll_id + " .poll-radio-wraper .radio").hide();
        $("#Poll_" + data.poll_id + " .poll-radio-wraper .submit-div").hide();
        for (i = 0; i < data.data.length; i++) {
            $("#Poll_" + data.poll_id + " .poll-radio-wraper").append("<span>" + data.data[i].Answer + "</span><div style='width:100%;height:35px;text-align:right; margin-top: 3px;'><div style='background-color:#23af44;height:10px;width:" + data.data[i].Percentage + "%'></div><span style='font-size:11px;color:#23af44'>" + data.data[i].Percentage + "%</span></div>");
        }
    }
}