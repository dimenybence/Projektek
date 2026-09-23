class User < ApplicationRecord
  has_many :enrollments
  has_many :enrolled_courses, through: :enrollments, source: :course

  has_many :favorites
  has_many :favorite_courses, through: :favorites, source: :course

  has_one_attached :profile_picture

  has_secure_password

  validates :username, presence: true, uniqueness: true
  validates :email, presence: true, uniqueness: true
  validates :password, presence: true, confirmation: true

  def profile_picture_url
    profile_picture.attached? ? Rails.application.routes.url_helpers.rails_blob_url(profile_picture, only_path: true) : 'https://via.placeholder.com/150'
  end

end
