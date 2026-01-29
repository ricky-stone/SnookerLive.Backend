use('Matches');

const id = "2346-8-1";

const updates = {
  Status: 3,
  Distance: 0
};

db.getCollection('all').findOneAndUpdate(
  { _id: id },
  { $set: updates },
  { returnDocument: "after" }
);