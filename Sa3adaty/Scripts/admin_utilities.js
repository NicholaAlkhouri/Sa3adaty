function textCounter(field, field2, maxlimit) {
    var countfield = document.getElementById(field2);
    countfield.innerHTML = maxlimit - field.value.length;
    if (maxlimit < field.value.length)
        countfield.style.color = "#ff0000";
    else
        countfield.style.color = "#00bb00";
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