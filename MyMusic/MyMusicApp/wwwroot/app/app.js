'use strict';

angular.module('myMusic', ['ngRoute']).
config(['$locationProvider', '$routeProvider', function ($locationProvider, $routeProvider) {
  $locationProvider.html5Mode({
    enabled: true,
    requireBase: false
  });
  $routeProvider.when('/', {
    templateUrl: 'app/my-music/my-music.html',
    controller: 'MyMusicController',
    controllerAs: 'vm'
  });
}]);