var noteid = -1;
$(function () {
    $('#showNote').on('show.bs.modal', function (e) {
        console.log("denemeNote");
        var btn = $(e.relatedTarget);
        noteid = btn.data("note-id");
        $('#modal-note-body').load("/Note/NoteDetails/" + noteid)
    });
});