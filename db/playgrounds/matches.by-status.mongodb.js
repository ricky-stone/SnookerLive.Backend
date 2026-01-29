use('Matches');

const query = {
  _id: "2346-9-2"
};

const count = db.getCollection('all').countDocuments(query);

const results = db.getCollection('all')
  .find(query)
  .toArray();

({ count, results });