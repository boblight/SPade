/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package fypinputtestapp;

import java.util.Scanner;

/**
 *
 * @author tongliang
 */
public class FYPInputTestApp {

    /**
     * @param args the command line arguments
     */
    public static void main(String[] args) {
        try {
            System.out.println("Enter number 1: ");

            Scanner sc = new Scanner(System.in);
            int i = sc.nextInt();

            System.out.println("Enter number 2: ");
            int j = sc.nextInt();
            System.out.println("Their sum is : " + (i + j));
        } catch (Exception e) {
            System.out.println("error");
        }
    }//end of main

}
