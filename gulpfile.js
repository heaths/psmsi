'use strict';

var gulp = require('gulp'),
    msbuild = require('gulp-msbuild');

var argv = require('yargs')
  .option('configuration', {
    choices: ['debug', 'release'],
    default: 'debug',
    demand: true,
    type: 'string'
  })
  .argv;

gulp.task('build', function() {
  return gulp.src('Psmsi.sln')
    .pipe(msbuild({
        targets: ['Build'],
        configuration: argv.configuration,
        errorOnFail: true
      })
    );
});

gulp.task('clean', function() {
  return gulp.src('Psmsi.sln')
    .pipe(msbuild({
        targets: ['Clean'],
        configuration: argv.configuration,
        errorOnFail: true
      })
    );
});

gulp.task('default', ['build']);
