function get() {
    fetch('http://team2-openhack.eastus.cloudapp.azure.com/api/servers')
    .then(function(response) {

        response.json().then(p => show(p));
    })
}

function del(name){
    fetch('http://team2-openhack.eastus.cloudapp.azure.com/api/servers/' + name, {
        method: 'DELETE',
        body: formData
    })
    .then(response => console.log(response))
}

function add(name) {
    
    fetch('http://team2-openhack.eastus.cloudapp.azure.com/api/servers', {
        method: 'POST',
        body: { name: name }
    })
    .then(response => console.log(response))
}

  get();

  function show(serverList) {
      $('#row').html('');
    for(i=0; i<serverList.length; i++){
        var str = '<div class="col-xs-6 col-md-3 col-lg-3 no-padding">' +
    '<div class="panel panel-teal panel-widget border-right"><div class="row no-padding"><em class="fa fa-xl fa-server color-blue"></em>' +
            '<div class="large">' + serverList[i].name + '</div>' +
            '<div class="text-muted">' + serverList[i].endpoints.minecraft + '</div>' +
            '<div class="text-muted">' + serverList[i].endpoints.rcon + '</div></div>' +
            '<a class="fa fa-window-close fa-2x color-red remove" data-id="' + serverList[i].name + '" style="position: absolute;top: 0;right: 0;"></a>' +
    '</div></div>';
        $('#row').append(str);
    }

    $('.remove').click(function() {
        del($(this).attr('data-id'));
        get();
    });
    
    
  }

  $('#btn-todo').click(function(){
    add($('#btn-input').val());
    get();
})
