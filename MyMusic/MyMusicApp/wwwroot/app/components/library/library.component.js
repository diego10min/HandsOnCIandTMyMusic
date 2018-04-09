'use strict';

angular.module('myMusic')
  .component('library', {
    templateUrl: 'app/components/library/library.html',
    controllerAs: 'vm',
    bindings: {
      library: '<',
      playlist: '<',
      adicionar: '<',
      buscar: '<'
    },
    controller: function () {
      var vm = this;

      vm.isDisabled = function (id) {
        return vm.playlist && _.some(vm.playlist.playlistMusicas, {
          musicaId: id
        });
      }
    }
  });