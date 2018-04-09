'use strict';

angular.module('myMusic')
  .component('playlist', {
    templateUrl: 'app/components/playlist/playlist.html',
    controllerAs: 'vm',
    bindings: {
      playlist: '<',
      remover: '<',
      buscar: '<'
    },
    controller: function () {
      var vm = this;
    }
  });