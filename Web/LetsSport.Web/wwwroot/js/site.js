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
