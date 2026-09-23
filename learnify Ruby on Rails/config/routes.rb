Rails.application.routes.draw do
  root "sessions#new"
  get "register", to: "sessions#register", as: "register"
  post "register", to: "sessions#create_register"

  resources :courses, only: [:index]
  resources :courses do
    post :enroll_toggle, on: :member
    post :favorite_toggle, on: :member
  end
  resources :users do
    member do
      patch :update_profile_picture
    end
  end

  post "login", to: "sessions#create_login"
  delete "logout", to: "sessions#destroy"
  get "account", to: "users#account"
  get "contact", to: "contact#index"
end