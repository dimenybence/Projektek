class Course < ApplicationRecord
  has_many :enrollments
  has_many :enrolled_users, through: :enrollments, source: :user

  has_many :favorites
  has_many :favorite_courses, through: :favorites, source: :course
end
