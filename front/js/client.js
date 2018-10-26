function get() {
    fetch('http://team2-openhack.eastus.cloudapp.azure.com/api/servers')
    .then(function(response) {

        response.json().then(p => show(p));
    })
}

function del(name){
    fetch('http://team2-openhack.eastus.cloudapp.azure.com/api/servers?serverName=' + name, {
        method: 'DELETE'
    })
    .then(response => get())
}

function add(name) {
    
    fetch('http://team2-openhack.eastus.cloudapp.azure.com/api/servers?serverName=' + name, {
        method: 'POST',
    })
    .then(response => get())
}

  get();

  function show(serverList) {
      $('#row').html('');
    for(i=0; i<serverList.length; i++){
        var str = '<div class="col-xs-6 col-md-3 col-lg-3 no-padding">' +
            '<script src="https://cdn.jsdelivr.net/gh/leonardosnt/mc-player-counter@1.1.1/dist/mc-player-counter.min.js"></script>' +
    '<div class="panel panel-teal panel-widget border-right"><div class="row no-padding"><em class="fa fa-xl fa-server color-blue"></em>' +
            '<div class="large">' + serverList[i].name + '</div>' +
            '<div class="text-muted">' + serverList[i].endpoints.minecraft + '</div>' +
            '<div class="text-muted">' + serverList[i].endpoints.rcon + '</div></div>' +
            '<div class="text-muted">players online/max: <span data-playercounter-ip="' + serverList[i].endpoints.minecraft + '" data-playercounter-format="{online}/{max}">0</span></div></div>' +
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

setInterval(get, 5000);