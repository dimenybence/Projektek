class SessionsController < ApplicationController
  def new
    redirect_to courses_path if user_signed_in?
  end

  def register
    @user = User.new
  end

  def create_login
  user = User.find_by(email: params[:email])

  if user && user.authenticate(params[:password])
    session[:user_id] = user.id
    redirect_to courses_path, notice: 'Sikeres bejelentkezés.'
  else
    flash.now[:alert] = user.nil? ? "A megadott email-cím nem található." : "Helytelen jelszó."
    render :new, status: :unprocessable_entity
  end
  end

  def create_register
    @user = User.new(user_params)
    if @user.save
      flash[:notice] = "Sikeres regisztráció!"
      redirect_to root_path, notice: 'Sikeres regisztráció.'
    else
      flash.now[:alert] = 'Regisztráció sikertelen.'
      render :new, status: :unprocessable_entity
    end
  end

  def destroy
    reset_session
    flash[:notice] = "Sikeres kijelentkezés!"
    redirect_to root_path, notice: 'Kiléptél.'
  end

  private

  def user_params
    params.permit(:username, :email, :password, :password_confirmation)
  end

  def user_signed_in?
    session[:user_id].present?
  end
end
