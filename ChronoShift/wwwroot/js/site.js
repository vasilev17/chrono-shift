

function CheckRole() {

    var siteUrl = location.protocol + '//' + location.host + "/CheckRole";

    $.ajax({
        url: siteUrl,
        data: null,
        cache: false,
        type: "GET",
        timeout: 10000,
        dataType: "json",
        success: function (result) {
            if (result.showPopUp) {
                $("#roleModal").modal('toggle');
            }
        }
    });
}


function SelectRole(id) {
    var siteUrl = location.protocol + '//' + location.host + "/SelectRole";
    $.ajax({
        url: siteUrl,
        data: { role: id }
    });

}


function CalculateWorkingDays(month, year) {

    var siteUrl = location.protocol + '//' + location.host + "/Holiday";

    $.ajax({
        url: siteUrl,
        data:
        {
            month: month,
            year: year
        },
        cache: false,
        type: "GET",
        timeout: 10000,
        dataType: "json",
        success: function (result) {
            if (result.workedDays) {
                document.getElementById("holiday").innerHTML = result.workedDays;
                console.log(result.workedDays);
            }
        }
    });
}




//Function to Save data from table to DB
function Save(rowNumber) {

    var activitySource = document.getElementById("row-" + rowNumber).querySelectorAll(".textarea");
    var timeSource = document.getElementById("row-" + rowNumber).querySelectorAll(".timePicker");

    var activityInput = activitySource[0].value;
    var timeInput = timeSource[0].value;

    if (activityInput != "" && timeInput != "") {
        var siteUrl = location.protocol + '//' + location.host + "/SaveTableData";
        $.ajax({
            url: siteUrl,
            data: {
                activityNum: 1,
                date: "02/01/2021",
                time: timeInput,
                activityDescription: activityInput

            }
        });
    }
   
}

function addActivity() {

    console.log(uuidv4()); // ⇨ '1b9d6bcd-bbfd-4b2d-9b5d-ab8dfbbd4bed'
    const guid = uuidv4();
    const text_area = '<hr style="margin:0px" /> <textarea data-autoresize onblur="Save(3)" class="textarea activity-input" role="textbox" contenteditable></textarea>';
    const timepicker = `<hr style="margin:0px" /> <input class="form-control example" type="text" id = "${guid}"  >`;
    document.getElementById("plus_btn_activity_02").insertAdjacentHTML("beforeend", text_area);
    document.getElementById("plus_btn_time_02").insertAdjacentHTML("beforeend", timepicker);
    $(`#${guid}`).timepicker({
        minStep: 15
    });

};

//New Activity "+" button functionality on click
document.querySelector(' .plus-btn').addEventListener('click', addActivity);


//Select role functionality on click

document.querySelector('#select-intern-btn').addEventListener('click', function () {
    SelectRole(1);
});

document.querySelector('#select-mentor-btn').addEventListener('click', function () {
    SelectRole(2);
});

document.querySelector('#select-manager-btn').addEventListener('click', function () {
    SelectRole(3);
});

//textarea auto-resize
jQuery.each(jQuery('textarea[data-autoresize]'), function () {
    var offset = this.offsetHeight - this.clientHeight;

    var resizeTextarea = function (el) {
        jQuery(el).css('height', 'auto').css('height', el.scrollHeight + offset);
    };
    jQuery(this).on('keyup input', function () { resizeTextarea(this); }).removeAttr('data-autoresize');
});