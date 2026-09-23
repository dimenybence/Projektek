# This file should ensure the existence of records required to run the application in every environment (production,
# development, test). The code here should be idempotent so that it can be executed at any point in every environment.
# The data can then be loaded with the bin/rails db:seed command (or created alongside the database with db:setup).
#
# Example:
#
#   ["Action", "Comedy", "Drama", "Horror"].each do |genre_name|
#     MovieGenre.find_or_create_by!(name: genre_name)
#   end
# db/seeds.rb
user1 = User.create!(
  username: "teszt_felhasznalo",
  email: "test@example.com",
  password: "password123",
  password_confirmation: "password123"
)

user2 = User.create!(
  username: "admin",
  email: "admin@example.com",
  password: "admin123",
  password_confirmation: "admin123"
)

# Kurzusok létrehozása
course1 = Course.create!(
  name: "Ruby kezdőknek",
  instructor: "Jóska Pista",
  difficulty_level: 1,
  description: "Tanuld meg a Ruby alapjait lépésről lépésre!"
)

course2 = Course.create!(
  name: "Webfejlesztés Rails-szel",
  instructor: "Kovács Ádám",
  difficulty_level: 3,
  description: "Fedezd fel, hogyan készíthetsz modern webalkalmazásokat a Rails keretrendszerrel."
)

course3 = Course.create!(
  name: "Haladó Ruby programozási technikák",
  instructor: "Nagy Béla",
  difficulty_level: 5,
  description: "Hozd ki a legtöbbet a Ruby nyelvből haladó szinten!"
)

Enrollment.create!(user: user1, course: course1)
Enrollment.create!(user: user1, course: course2)
Enrollment.create!(user: user2, course: course3)


Favorite.create!(user: user1, course: course3)
Favorite.create!(user: user2, course: course1)
Favorite.create!(user: user2, course: course2)

puts "Tesztadatok sikeresen létrehozva!"