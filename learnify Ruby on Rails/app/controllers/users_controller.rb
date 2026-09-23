class UsersController < ApplicationController
  def new
    @user = User.new
  end

  def account
    @user = current_user
    @courses = @user.enrolled_courses
    @favorites = @user.favorite_courses
  end

  def favorite_course
    course = Course.find(params[:course_id])
    if current_user.favorite_courses.include?(course)
      flash[:alert] = "Ez a kurzus már a kedvenceid között van!"
    else
      current_user.favorite_courses << course
      flash[:notice] = "A kurzust hozzáadtad a kedvenceidhez!"
    end
    redirect_to account_path
  end

  def update_profile_picture
    @user = User.find(params[:id])

    if params[:user] && params[:user][:profile_picture].present?
      if @user.update(profile_picture: params[:user][:profile_picture])
        redirect_to account_path, notice: 'Profilkép sikeresen frissítve!'
      else
        redirect_to account_path, alert: 'Hiba történt a profilkép frissítése során.'
      end
    else
      redirect_to account_path, alert: 'Nem adott meg képet a feltöltéshez!'
    end
  end

  private

  def user_params
    params.require(:user).permit(:username, :email, :password)
  end
end