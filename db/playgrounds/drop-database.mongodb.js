/* global use, db */

use('Matches');

const result = db.dropDatabase();
console.log('Dropped database Matches:', result);