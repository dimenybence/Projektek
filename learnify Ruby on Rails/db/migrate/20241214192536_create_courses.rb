class CreateCourses < ActiveRecord::Migration[7.2]
  def change
    create_table :courses do |t|
      t.string :name
      t.string :instructor
      t.integer :difficulty_level
      t.text :description

      t.timestamps
    end
  end
end
