// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// confirmation messages fading out
setTimeout(function () {
    $(document).ready(function () {
        $('#confirmation-message').fadeOut(3000);
    });
}, 5000);

// chat room code
var connection =
    new signalR.HubConnectionBuilder()
        .withUrl("/events/details")
        .configureLogging(signalR.LogLevel.Information)
        .build();

connection.on("NewMessage",
    function (message) {
        var chatInfo =
            `<div class="d-flex justify-content-center mb-3">
                                            <div class="img_cont_msg">
                                                <img src="${message.senderAvatarUrl}" asp-append-version="true" class="rounded-circle user_img_msg" alt="${message.SenderUserName} Avatar">
                                                <div class="img_cont_msg">${message.senderUserName}</div>
                                            </div>
                                            <div class="msg_cotainer_new">
                                                ${message.content}
                                                <span class="msg_time" style="display: block">
                                                    <time >${message.createdOn}</time>
                                                </span>
                                            </div>
                                        </div>`;
        $("#messagesList").prepend(chatInfo);
    });

$("#sendButton").click(function () {
    var message = $("#messageInput").val();
    if (message == '') {
        $("#messageInput").val("");
        return;
    }
    var eventId = $("#event-id").val();
    var userId = $("#user-id").val();
    $("#messageInput").val("");

    connection.invoke('SendMessage', message, eventId, userId);
});

//connection.start().catch(function (err) {
//    return console.error(err.toString());
//});

function escapeHtml(unsafe) {
    return unsafe
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}



//$(function () {
//    $('[datetime]').each(function () {
//        var value = moment($(this).attr('datetime'));
//        if (!value) {
//            return;
//        }
//        var local = moment.utc(value).local().format("HH:mm");
//        $(this).html(local);
//    });
//});
