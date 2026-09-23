class CoursesController < ApplicationController
  before_action :authenticate_user!

  def index
    @courses = Course.all

    if params[:name].present?
      @courses = @courses.where("LOWER(name) LIKE ?", "%#{params[:name].downcase}%")
    end

    if params[:instructor].present?
      @courses = @courses.where("LOWER(instructor) LIKE ?", "%#{params[:instructor].downcase}%")
    end

    if params[:difficulty_level].present?
      @courses = @courses.where(difficulty_level: params[:difficulty_level])
    end
  end

  def enroll_toggle
    @course = Course.find(params[:id])
    if current_user.enrolled_courses.include?(@course)
      current_user.enrolled_courses.delete(@course)
      flash[:notice] = "Sikeresen lejelentkeztél a kurzusról."
    else
      current_user.enrolled_courses << @course
      flash[:notice] = "Sikeresen beiratkoztál a kurzusra."
    end
    redirect_to courses_path
  end

  def favorite_toggle
    @course = Course.find(params[:id])
    if current_user.favorite_courses.include?(@course)
      current_user.favorite_courses.delete(@course)
      flash[:alert] = "Eltávolítottad a kurzust a kedvenceid közül!"
    else
      current_user.favorite_courses << @course
      flash[:notice] = "A kurzust hozzáadtad a kedvenceidhez!"
    end
    redirect_to courses_path
  end
end